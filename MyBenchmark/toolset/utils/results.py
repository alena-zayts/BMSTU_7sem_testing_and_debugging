from toolset.utils.output_helper import log
from toolset.test_types import test_types

import os
import subprocess
import uuid
import time
import json
import requests
import threading
import re
import math
import csv
import traceback
from datetime import datetime

# Cross-platform colored text
from colorama import Fore, Style


class Results:
    def __init__(self, benchmarker):
        self.benchmarker = benchmarker
        self.config = benchmarker.config
        self.directory = self.config.results_directory
        try:
            os.makedirs(self.directory)
        except OSError:
            pass
        self.file = os.path.join(self.directory, "results.json")
        self.name = datetime.now().strftime('(happened at datetime = %Y-%m-%d %H:%M:%S)')

        self.startTime = int(round(time.time() * 1000))
        self.completionTime = None
        self.jsonConcurrencyLevels = self.config.json_concurrency_levels
        self.plaintextConcurrencyLevels = self.config.plaintext_concurrency_levels
        self.queryLevels = self.config.query_levels
        self.frameworks = [t.name for t in benchmarker.tests]
        self.duration = self.config.duration
        self.rawData = dict()
        self.completed = dict()
        self.succeeded = dict()
        self.failed = dict()
        for type in test_types:
            self.rawData[type] = dict()
            self.failed[type] = []
            self.succeeded[type] = []

    #############################################################################
    # PUBLIC FUNCTIONS
    #############################################################################

    def parse_test(self, framework_test, test_type):
        """
        Parses the given test and test_type from the raw_file.
        """
        results = dict()
        results['results'] = []
        stats = []

        if os.path.exists(self.get_raw_file(framework_test.name, test_type)):
            with open(self.get_raw_file(framework_test.name, test_type)) as raw_data:

                is_warmup = True
                rawData = None
                for line in raw_data:
                    if "Queries:" in line or "Concurrency:" in line:
                        is_warmup = False
                        rawData = None
                        continue
                    if "Warmup" in line or "Primer" in line:
                        is_warmup = True
                        continue
                    if not is_warmup:
                        if rawData is None:
                            rawData = dict()
                            results['results'].append(rawData)
                        if "Latency" in line:
                            m = re.findall(r"([0-9]+\.*[0-9]*[us|ms|s|m|%]+)", line)
                            if len(m) == 4:
                                rawData['latencyAvg'] = m[0]
                                rawData['latencyStdev'] = m[1]
                                rawData['latencyMax'] = m[2]
                        if "requests in" in line:
                            m = re.search("([0-9]+) requests in", line)
                            if m is not None:
                                rawData['totalRequests'] = int(m.group(1))
                        if "Socket errors" in line:
                            if "connect" in line:
                                m = re.search("connect ([0-9]+)", line)
                                rawData['connect'] = int(m.group(1))
                            if "read" in line:
                                m = re.search("read ([0-9]+)", line)
                                rawData['read'] = int(m.group(1))
                            if "write" in line:
                                m = re.search("write ([0-9]+)", line)
                                rawData['write'] = int(m.group(1))
                            if "timeout" in line:
                                m = re.search("timeout ([0-9]+)", line)
                                rawData['timeout'] = int(m.group(1))
                        if "Non-2xx" in line:
                            m = re.search("Non-2xx or 3xx responses: ([0-9]+)", line)
                            if m != None:
                                rawData['5xx'] = int(m.group(1))
                        if "STARTTIME" in line:
                            m = re.search("[0-9]+", line)
                            rawData["startTime"] = int(m.group(0))
                        if "ENDTIME" in line:
                            m = re.search("[0-9]+", line)
                            rawData["endTime"] = int(m.group(0))

        return results

    def write_intermediate(self, test_name, status_message):
        self.completed[test_name] = status_message
        self.__write_results()

    def set_completion_time(self):
        self.completionTime = int(round(time.time() * 1000))
        self.__write_results()

    def load(self):
        try:
            with open(self.file) as f:
                self.__dict__.update(json.load(f))
        except (ValueError, IOError):
            pass

    def get_raw_file(self, test_name, test_type):
        """
        Returns the output file for this test_name and test_type
        Example: fw_root/results/timestamp/test_type/test_name/raw.txt
        """
        path = os.path.join(self.directory, test_name, test_type, "raw.txt")
        try:
            os.makedirs(os.path.dirname(path))
        except OSError:
            pass
        return path

    def report_benchmark_results(self, framework_test, test_type, results):
        '''
        Used by FrameworkTest to add benchmark data to this

        TODO: Technically this is an IPC violation - we are accessing
        the parent process' memory from the child process
        '''
        if test_type not in self.rawData.keys():
            self.rawData[test_type] = dict()

        # If results has a size from the parse, then it succeeded.
        if results:
            self.rawData[test_type][framework_test.name] = results

            # This may already be set for single-tests
            if framework_test.name not in self.succeeded[test_type]:
                self.succeeded[test_type].append(framework_test.name)
        else:
            # This may already be set for single-tests
            if framework_test.name not in self.failed[test_type]:
                self.failed[test_type].append(framework_test.name)

    def finish(self):
        log('', border='=', border_bottom='', color=Fore.CYAN)
        log("Results are saved in " + self.directory)

    #############################################################################
    # PRIVATE FUNCTIONS
    #############################################################################

    def __to_jsonable(self):
        '''
        Returns a dict suitable for jsonification
        '''
        toRet = dict()

        toRet['name'] = self.name
        toRet['startTime'] = self.startTime
        toRet['completionTime'] = self.completionTime
        toRet['jsonConcurrencyLevels'] = self.jsonConcurrencyLevels
        toRet['plaintextConcurrencyLevels'] = self.plaintextConcurrencyLevels
        toRet['queryLevels'] = self.queryLevels
        toRet['frameworks'] = self.frameworks
        toRet['duration'] = self.duration
        toRet['rawData'] = self.rawData
        toRet['completed'] = self.completed
        toRet['succeeded'] = self.succeeded
        toRet['failed'] = self.failed

        return toRet

    def __write_results(self):
        try:
            with open(self.file, 'w') as f:
                f.write(json.dumps(self.__to_jsonable(), indent=2))
        except IOError:
            log("Error writing results.json")