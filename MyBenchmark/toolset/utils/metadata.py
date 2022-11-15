import os
import glob
import json

from collections import OrderedDict
from colorama import Fore
from toolset.databases import databases
from toolset.utils.output_helper import log


class Metadata:
    supported_dbs = [(name, '...') for name in databases]

    def __init__(self, benchmarker):
        self.benchmarker = benchmarker


    def gather_tests(self, include=None):
        # Given test names as strings, returns a list of FrameworkTest objects.

        # Help callers out a bit
        include = include or []

        # Search for configuration files
        config_files = glob.glob("{!s}/*/benchmark_config.json".format(self.benchmarker.config.frameworks_root))

        tests = []
        for config_file_name in config_files:
            with open(config_file_name, 'r') as config_file:
                config = json.load(config_file)
            # Find all tests in the config file
            config_tests = self.parse_config(config, os.path.dirname(config_file_name))

            # Filter
            for test in config_tests:
                if len(include) == 0 or test.name in include:
                    tests.append(test)

        # Ensure we were able to locate everything that was
        # explicitly included
        if len(include) and len(include) != len(tests):
            raise Exception("Not all tests")

        tests.sort(key=lambda x: x.name)

        return tests

    # Gathers all tests for current benchmark run.
    def tests_to_run(self):
        return self.gather_tests(self.benchmarker.config.test)

    # Parses a config file into a list of FrameworkTest objects
    def parse_config(self, config, directory):
        from toolset.benchmark.framework_test import FrameworkTest

        test_name, test_keys = config['framework'], config['test']
        test_keys['project_name'] = config['framework']

        # Map test type to a parsed FrameworkTestType object
        runTests = dict()
        for type_name, type_obj in self.benchmarker.config.types.iteritems():
            try:
                runTests[type_name] = type_obj.copy().parse(test_keys)
            except AttributeError:
                # This is quite common - most tests don't support all types
                pass

        # don't delete
        sortedTestKeys = sorted(runTests.keys(), key=lambda x: len(x))
        sortedRunTests = OrderedDict()
        for sortedTestKey in sortedTestKeys:
            sortedRunTests[sortedTestKey] = runTests[sortedTestKey]

        # By passing the entire set of keys, each FrameworkTest will have a member for each key
        tests = [FrameworkTest(directory, self.benchmarker, sortedRunTests, test_keys)]

        return tests

    def to_jsonable(self):
        '''
        Returns an array suitable for jsonification
        '''
        all_tests = self.gather_tests()
        return map(lambda test: {
            "project_name": test.project_name,
            "name": test.name,
            "database": test.database,
            "framework": test.framework,
        }, all_tests)

    # Prints the metadata for all the available tests
    def list_test_metadata(self):
        all_tests_json = json.dumps(self.to_jsonable())

        with open(os.path.join(self.benchmarker.results.directory, "test_metadata.json"), "w") as f:
            f.write(all_tests_json)


