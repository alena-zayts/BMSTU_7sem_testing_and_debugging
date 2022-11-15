from toolset.utils.output_helper import QuietOutputStream
from toolset.test_types import test_types

import os
import time
import socket


class BenchmarkConfig:
    def __init__(self, args):
        '''
        Configures this BenchmarkConfig given the arguments provided.
        '''

        # Map type strings to their objects
        types = {}
        for type in test_types:
            types[type] = test_types[type](self)  # Abstract test type

        # Turn type into a map instead of a list of strings
        if 'all' in args.type:
            self.types = types
        else:
            self.types = {t: types[t] for t in args.type}

        self.duration = 15
        self.server_host = 'bm-server'
        self.database_host = 'bm-database'
        self.list_tests = args.list_tests

        # self.concurrency_levels = [16, 32, 64, 128, 256, 512]
        # self.pipeline_concurrency_levels = [256, 1024, 4096, 16384]
        # self.query_levels = [1, 5, 10, 15, 20]
        self.concurrency_levels = [16, ]
        self.pipeline_concurrency_levels = [256, ]
        self.query_levels = [1, ]
        self.max_concurrency = max(self.concurrency_levels)
        self.results_environment = '(unspecified, hostname = %s)' % socket.gethostname()
        self.results_name = '(unspecified, datetime = %Y-%m-%d %H:%M:%S)'
        self.test = args.test

        self.network = 'bm'
        self.server_docker_host = "unix://var/run/docker.sock"
        self.database_docker_host = "unix://var/run/docker.sock"
        self.client_docker_host = "unix://var/run/docker.sock"

        self.quiet_out = QuietOutputStream(False)

        # Remember directories
        self.fw_root = os.getenv('FWROOT')
        self.db_root = os.path.join(self.fw_root, "toolset", "databases")
        self.frameworks_root = os.path.join(self.fw_root, "frameworks")
        self.results_root = os.path.join(self.fw_root, "results")
        self.wrk_root = os.path.join(self.fw_root, "toolset", "wrk")

        self.timestamp = time.strftime("%Y%m%d%H%M%S", time.localtime())
