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
        self.concurrencyLevels = self.config.concurrency_levels
        self.pipelineConcurrencyLevels = self.config.pipeline_concurrency_levels
        self.queryIntervals = self.config.query_levels
        self.frameworks = [t.name for t in benchmarker.tests]
        self.duration = self.config.duration
        self.rawData = dict()
        self.completed = dict()
        self.succeeded = dict()
        self.failed = dict()
        self.verify = dict()
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
            with open(self.get_raw_file(framework_test.name,
                                        test_type)) as raw_data:

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
                            m = re.findall(r"([0-9]+\.*[0-9]*[us|ms|s|m|%]+)",
                                           line)
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
                            m = re.search("Non-2xx or 3xx responses: ([0-9]+)",
                                          line)
                            if m != None:
                                rawData['5xx'] = int(m.group(1))
                        if "STARTTIME" in line:
                            m = re.search("[0-9]+", line)
                            rawData["startTime"] = int(m.group(0))
                        if "ENDTIME" in line:
                            m = re.search("[0-9]+", line)
                            rawData["endTime"] = int(m.group(0))
                            test_stats = self.__parse_stats(
                                framework_test, test_type,
                                rawData["startTime"], rawData["endTime"], 1)
                            stats.append(test_stats)
        with open(self.get_stats_file(framework_test.name, test_type) + ".json", "w") as stats_file:
            json.dump(stats, stats_file, indent=2)

        return results

    def write_intermediate(self, test_name, status_message):
        '''
        Writes the intermediate results for the given test_name and status_message
        '''
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
        '''
        Returns the output file for this test_name and test_type
        Example: fw_root/results/timestamp/test_type/test_name/raw.txt
        '''
        path = os.path.join(self.directory, test_name, test_type, "raw.txt")
        try:
            os.makedirs(os.path.dirname(path))
        except OSError:
            pass
        return path

    def get_stats_file(self, test_name, test_type):
        '''
        Returns the stats file name for this test_name and
        Example: fw_root/results/timestamp/test_type/test_name/stats.txt
        '''
        path = os.path.join(self.directory, test_name, test_type, "stats.txt")
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
        toRet['concurrencyLevels'] = self.concurrencyLevels
        toRet['pipelineConcurrencyLevels'] = self.pipelineConcurrencyLevels
        toRet['queryIntervals'] = self.queryIntervals
        toRet['frameworks'] = self.frameworks
        toRet['duration'] = self.duration
        toRet['rawData'] = self.rawData
        toRet['completed'] = self.completed
        toRet['succeeded'] = self.succeeded
        toRet['failed'] = self.failed
        toRet['verify'] = self.verify

        return toRet

    def __write_results(self):
        try:
            with open(self.file, 'w') as f:
                f.write(json.dumps(self.__to_jsonable(), indent=2))
        except IOError:
            log("Error writing results.json")


    def __parse_stats(self, framework_test, test_type, start_time, end_time,
                      interval):
        '''
        For each test type, process all the statistics, and return a multi-layered
        dictionary that has a structure as follows:

        (timestamp)
        | (main header) - group that the stat is in
        | | (sub header) - title of the stat
        | | | (stat) - the stat itself, usually a floating point number
        '''
        stats_dict = dict()
        stats_file = self.get_stats_file(framework_test.name, test_type)
        with open(stats_file) as stats:
            # dstat doesn't output a completely compliant CSV file - we need to strip the header
            for _ in range(4):
                stats.next()
            stats_reader = csv.reader(stats)
            main_header = stats_reader.next()
            sub_header = stats_reader.next()
            time_row = sub_header.index("epoch")
            int_counter = 0
            for row in stats_reader:
                time = float(row[time_row])
                int_counter += 1
                if time < start_time:
                    continue
                elif time > end_time:
                    return stats_dict
                if int_counter % interval != 0:
                    continue
                row_dict = dict()
                for nextheader in main_header:
                    if nextheader != "":
                        row_dict[nextheader] = dict()
                header = ""
                for item_num, column in enumerate(row):
                    if len(main_header[item_num]) != 0:
                        header = main_header[item_num]
                    # all the stats are numbers, so we want to make sure that they stay that way in json
                    row_dict[header][sub_header[item_num]] = float(column)
                stats_dict[time] = row_dict
        return stats_dict
