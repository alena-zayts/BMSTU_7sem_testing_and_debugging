from toolset.utils.output_helper import QuietOutputStream
from toolset.test_types import test_types

import os
import time


class BenchmarkConfig:
    def __init__(self, args):
        '''
        Configures this BenchmarkConfig given the arguments provided.
        '''

        # Map type strings to their objects
        types = {}
        for type in test_types:
            types[type] = test_types[type](self)

        # Turn type into a map instead of a list of strings
        if 'all' in args.type:
            self.types = types
        else:
            self.types = {t: types[t] for t in args.type}

        self.duration = 15
        self.exclude = []
        self.server_host = 'tfb-server'
        self.database_host = 'tfb-database'
        self.client_host = ''
        self.mode = 'benchmark'
        self.list_tests = args.list_tests

        # self.concurrency_levels = [16, 32, 64, 128, 256, 512]
        # self.pipeline_concurrency_levels = [256, 1024, 4096, 16384]
        # self.query_levels = [1, 5, 10, 15, 20]
        self.concurrency_levels = [16, ]
        self.pipeline_concurrency_levels = [256, ]
        self.query_levels = [1, 5, ]
        self.max_concurrency = max(self.concurrency_levels)
        self.results_environment = args.results_environment
        self.results_name = '(unspecified, datetime = %Y-%m-%d %H:%M:%S)'
        self.results_upload_uri = None
        self.test = args.test
        self.test_dir = None
        self.test_lang = None
        self.tag = None
        self.network_mode = None

        self.network = 'tfb'
        self.server_docker_host = "unix://var/run/docker.sock"
        self.database_docker_host = "unix://var/run/docker.sock"
        self.client_docker_host = "unix://var/run/docker.sock"

        self.quiet_out = QuietOutputStream(False)

        self.start_time = time.time()

        # Remember directories
        self.fw_root = os.getenv('FWROOT')
        self.db_root = os.path.join(self.fw_root, "toolset", "databases")
        self.lang_root = os.path.join(self.fw_root, "frameworks")
        self.results_root = os.path.join(self.fw_root, "results")
        self.wrk_root = os.path.join(self.fw_root, "toolset", "wrk")

        self.timestamp = time.strftime("%Y%m%d%H%M%S", time.localtime())

        self.run_test_timeout_seconds = 7200
