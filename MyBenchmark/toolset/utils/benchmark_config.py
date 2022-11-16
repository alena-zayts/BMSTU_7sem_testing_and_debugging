from toolset.utils.output_helper import QuietOutputStream
from toolset.test_types import test_types

import os
import time
import socket



class BenchmarkConfig:
    def __init__(self, args):
        # Map type strings to their objects
        all_available_types = {}
        for type in test_types:
            all_available_types[type] = test_types[type](self)  # Abstract test type
        if 'all' in args.type:
            self.types = all_available_types
        else:
            self.types = {t: all_available_types[t] for t in args.type}

        self.test = args.test

        self.duration = 15  # for wrk
        self.server_host = 'bm-server'  # for urls
        self.database_host = 'bm-database'  # for db: db = psycopg2.connect(host=config.database_host, ...

        # for docker (containers run in one network)
        self.network = 'bm'
        self.server_docker_host = "unix://var/run/docker.sock"
        self.database_docker_host = "unix://var/run/docker.sock"
        self.client_docker_host = "unix://var/run/docker.sock"
        self.results_environment = '(unspecified, hostname = %s)' % socket.gethostname()  # buildargs

        self.quiet_out = QuietOutputStream(False)

        # Remember directories
        self.fw_root = os.getenv('FWROOT')
        self.db_root = os.path.join(self.fw_root, "toolset", "databases")  # for db and docker
        self.frameworks_root = os.path.join(self.fw_root, "frameworks")  # to collect config files
        self.wrk_root = os.path.join(self.fw_root, "toolset", "wrk")  # to build a container for wrk
        self.results_directory = os.path.join(os.path.join(self.fw_root, "results"),
                                              time.strftime("%Y%m%d%H%M%S", time.localtime()))


        # self.concurrency_levels = [16, 32, 64, 128, 256, 512]
        # self.pipeline_concurrency_levels = [256, 1024, 4096, 16384]
        # self.query_levels = [1, 5, 10, 15, 20]

        self.concurrency_levels = [16, 32]
        self.pipeline_concurrency_levels = [256, ]
        self.query_levels = [1, 5, 10]
