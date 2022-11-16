from toolset.utils.output_helper import log, FNULL
from toolset.utils.docker_helper import DockerHelper
from toolset.utils.time_logger import TimeLogger
from toolset.utils.results import Results

import os
import subprocess
import traceback
import sys
import time
import shlex
from pprint import pprint
from colorama import Fore
import glob
import json
from collections import OrderedDict
import copy


class Benchmarker:
    def __init__(self, config):
        self.config = config
        self.time_logger = TimeLogger()

        # a list of all tests for this run
        self.tests = self._gather_tests(self.config.test)

        self.results = Results(self)
        self.docker_helper = DockerHelper(self)

    ##########################################################################################
    # Public methods
    ##########################################################################################

    def run(self):
        """
        set up the client/server machines with any necessary change.
        go through each test, running their docker build and run, running benchmarks against them.
        """
        any_failed = False
        # Run tests
        log("Running Tests...", border='=')

        self.docker_helper.build_wrk()
        self.docker_helper.build_database()

        with open(os.path.join(self.results.directory, 'benchmark.log'), 'w') as benchmark_log:
            for test in self.tests:
                log("Running Test: %s" % test.name, border='-')
                with self.config.quiet_out.enable():
                    if not self.__run_test(test, benchmark_log):
                        any_failed = True

                # Load intermediate result from child process
                self.results.load()

        # Parse results
        log("Parsing Results ...", border='=')
        self.results.set_completion_time()
        self.results.finish()

        return any_failed

    def stop(self, signal=None, frame=None):
        log("Shutting down (may take a moment)")
        self.docker_helper.stop()
        sys.exit(0)

    ##########################################################################################
    # Private methods
    ##########################################################################################

    def __exit_test(self, success, prefix, file, message=None):
        if message:
            log(message, prefix=prefix, file=file, color=Fore.RED if success else '')
        self.time_logger.log_test_end(log_prefix=prefix, file=file)

        # Sleep for 60 seconds to ensure all host connects are closed
        log("Clean up: Sleep 60 seconds...", prefix=prefix, file=file)
        time.sleep(60)
        # After benchmarks are complete for all test types in this test,
        # let's clean up leftover test images (my_bm/bm.test.test-name)
        self.docker_helper.clean()

        return success

    def __run_test(self, test, benchmark_log):
        '''
        Runs the given test, verifies that the webapp is accepting requests,
        optionally benchmarks the webapp, and  stops all services started for this test.
        '''

        log_prefix = "%s: " % test.name
        self.time_logger.mark_test_start()

        log("Benchmarking %s" % test.name, file=benchmark_log, border='-')

        try:
            # Start database container
            if test.database.lower() != "none":
                self.time_logger.mark_starting_database()
                database_container = self.docker_helper.start_database(test.database.lower())

                if database_container is None:
                    message = "ERROR: Problem building/running database container"
                    self.results.write_intermediate(test.name, message)
                    return self.__exit_test(success=False, message=message, prefix=log_prefix, file=benchmark_log)

                self.time_logger.mark_started_database()

            # Start webapp
            container = test.start()
            self.time_logger.mark_test_starting()

            if container is None:
                message = "ERROR: Problem starting {name}".format(name=test.name)
                self.results.write_intermediate(test.name, message)
                return self.__exit_test(success=False, message=message, prefix=log_prefix, file=benchmark_log)

            max_time = time.time() + 60
            while True:
                accepting_requests = test.is_accepting_requests()
                if accepting_requests or time.time() >= max_time \
                        or not self.docker_helper.server_container_exists(container.id):
                    break
                time.sleep(1)

            # TODO delete
            # if hasattr(test, 'wait_before_sending_requests') and isinstance(test.wait_before_sending_requests, numbers.Integral) and test.wait_before_sending_requests > 0:
            #     time.sleep(test.wait_before_sending_requests)

            if not accepting_requests:
                message = "ERROR: Framework is not accepting requests from client machine"
                self.results.write_intermediate(test.name, message)
                return self.__exit_test(success=False, message=message, prefix=log_prefix, file=benchmark_log)

            self.time_logger.mark_test_accepting_requests()

            # TODO delete
            # Verify URLs and audit
            # log("Verifying framework URLs", prefix=log_prefix)
            # self.time_logger.mark_verify_start()
            # passed_verify = test.verify_urls()

            # from toolset.utils.audit import Audit
            # self.audit = Audit(self)
            # self.audit.audit_test_dir(test.directory)

            # Benchmark this test
            self.time_logger.mark_benchmarking_start()
            self.__benchmark(test, benchmark_log)
            self.time_logger.log_benchmarking_end(log_prefix=log_prefix, file=benchmark_log)

            # Log test timing stats
            self.time_logger.log_build_flush(benchmark_log)
            self.time_logger.log_database_start_time(log_prefix, benchmark_log)
            self.time_logger.log_test_accepting_requests(log_prefix, benchmark_log)
            # self.time_logger.log_verify_end(log_prefix, benchmark_log)

            # Save results thus far into the latest results directory
            self.results.write_intermediate(test.name, time.strftime("%Y%m%d%H%M%S", time.localtime()))

        except Exception as e:
            tb = traceback.format_exc()
            self.results.write_intermediate(test.name, "error during test: " + str(e))
            log(tb, prefix=log_prefix, file=benchmark_log)
            return self.__exit_test( success=False, message="Error during test: %s" % test.name,
                                     prefix=log_prefix, file=benchmark_log)
        finally:
            self.docker_helper.stop()

        return self.__exit_test(success=True, prefix=log_prefix, file=benchmark_log)

    def __benchmark(self, framework_test, benchmark_log):
        for test_type, test in framework_test.runTests.items():
            log("BENCHMARKING %s ... " % test_type.upper(), file=benchmark_log)

            raw_file = self.results.get_raw_file(framework_test.name, test_type)
            if not os.path.exists(raw_file):
                # Open to create the empty file
                with open(raw_file, 'w'):
                    pass

            if not test.failed:
                script = self.config.types[test_type].get_script_name()
                script_variables = self.config.types[test_type].get_script_variables(
                    test.name,
                    "http://%s:%s%s" % (self.config.server_host, framework_test.port, test.get_url()))

                self.docker_helper.benchmark(script, script_variables, raw_file)

            results = self.results.parse_test(framework_test, test_type)
            log("Benchmark results:", file=benchmark_log)
            pprint(results)

            self.results.report_benchmark_results(framework_test, test_type, results['results'])
            log("Complete", file=benchmark_log)

    def _gather_tests(self, include):
        # Given test names as strings, returns a list of FrameworkTest objects.

        # Search for configuration files
        config_files = glob.glob("{!s}/*/benchmark_config.json".format(self.config.frameworks_root))

        tests = []
        for config_file_name in config_files:
            with open(config_file_name, 'r') as config_file:
                config = json.load(config_file)
            # Find all tests in the config file
            config_tests = self._parse_config(config, os.path.dirname(config_file_name))

            # Filter
            for test in config_tests:
                if test.name in include:
                    tests.append(test)

        if len(include) != len(tests):
            raise Exception("Not all tests")

        tests.sort(key=lambda x: x.name)

        return tests

    # Parses a config file into a list of FrameworkTest objects
    def _parse_config(self, config, directory):
        from toolset.benchmark.framework_test import FrameworkTest

        test_keys = config['test']

        # Map test type to a parsed FrameworkTestType object
        runTests = dict()
        for type_name, type_obj in self.config.types.iteritems():
            try:
                runTests[type_name] = copy.copy(type_obj).parse(test_keys)
            except AttributeError:
                # some tests don't support all types
                pass

        # don't delete
        sortedTestKeys = sorted(runTests.keys(), key=lambda x: len(x))
        sortedRunTests = OrderedDict()
        for sortedTestKey in sortedTestKeys:
            sortedRunTests[sortedTestKey] = runTests[sortedTestKey]

        # each FrameworkTest will have a member for each key
        tests = [FrameworkTest(directory, self, sortedRunTests, test_keys)]

        return tests

