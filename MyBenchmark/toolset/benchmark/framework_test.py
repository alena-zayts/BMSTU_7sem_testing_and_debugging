import os
import traceback
from requests import ConnectionError, Timeout

from toolset.utils.output_helper import log

# Cross-platform colored text
from colorama import Fore, Style


class FrameworkTest:
    def __init__(self, directory, benchmarker, runTests, args):
        self.name = args['framework']
        self.directory = directory
        self.benchmarker = benchmarker
        self.runTests = runTests
        self.database = ""
        self.port = ""
        self.__dict__.update(args)

    def start(self):
        test_log_dir = os.path.join(self.benchmarker.results.directory, self.name.lower())
        build_log_dir = os.path.join(test_log_dir, 'build')
        run_log_dir = os.path.join(test_log_dir, 'run')

        try:
            os.makedirs(build_log_dir)
            os.makedirs(run_log_dir)
        except OSError:
            pass

        if self.benchmarker.docker_helper.build(self, build_log_dir) != 0:
            return None

        return self.benchmarker.docker_helper.run(self, run_log_dir)

    # Determines whether this test implementation is up and accepting requests.
    def is_accepting_requests(self):
        test_type = None
        for any_type in self.runTests:
            test_type = any_type
            break

        url = "http://%s:%s%s" % (self.benchmarker.config.server_host, self.port, self.runTests[test_type].get_url())

        return self.benchmarker.docker_helper.test_client_connection(url)
