import json
import psycopg2
import traceback
import re
from colorama import Fore
from toolset.utils.output_helper import log
from toolset.utils.popen import PopenTimeout
import shlex
import subprocess

class Database:
    # margin of tolerance on the results (rows read or updated only)
    margin = 1.011

    @classmethod
    def get_connection(cls, config):
        db = psycopg2.connect(
                host=config.database_host,
                port="5432",
                user="benchmarkdbuser",
                password="benchmarkdbpass",
                database="hello_world")
        cursor = db.cursor()
        cursor.execute("CREATE EXTENSION IF NOT EXISTS pg_stat_statements")
        return db

    @classmethod
    def get_current_world_table(cls, config):
        results_json = []

        try:
            db = cls.get_connection(config)
            cursor = db.cursor()
            cursor.execute("SELECT * FROM \"World\"")
            results = cursor.fetchall()
            results_json.append(json.loads(json.dumps(dict(results))))
            cursor = db.cursor()
            cursor.execute("SELECT * FROM \"world\"")
            results = cursor.fetchall()
            results_json.append(json.loads(json.dumps(dict(results))))
            db.close()
        except Exception:
            tb = traceback.format_exc()
            log("ERROR: Unable to load current Postgres World table.",
                color=Fore.RED)
            log(tb)

        return results_json

    @classmethod
    def test_connection(cls, config):
        try:
            db = cls.get_connection(config)
            cursor = db.cursor()
            cursor.execute("SELECT 1")
            cursor.fetchall()
            db.close()
            return True
        except:
            return False

    @classmethod
    def __exec_and_fetchone(cls, config, query):
        db = cls.get_connection(config)
        cursor = db.cursor()
        cursor.execute(query)
        record = cursor.fetchone()
        return record[0]


    @classmethod
    def verify_queries(cls, config, table_name, url, concurrency=512, count=2, check_updates=False):
        '''
        Verify query and row numbers for table_name.
        Retrieve from the database statistics of the number of queries made, the number of rows read, eventually the number of updated rows.
        Run 2 repetitions of http requests at the concurrency level 512 with siege.
        Retrieve statistics again, calculate the number of queries made and the number of rows read.
        '''
        trans_failures = 0
        rows_updated = None
        cls.tbl_name = table_name # used for Postgres and mongodb

        queries = int(cls.get_queries(config))
        rows = int(cls.get_rows(config))
        if check_updates:
            rows_updated = int(cls.get_rows_updated(config))

        cls.reset_cache(config)
        #Start siege requests with timeout (20s)
        path = config.db_root
        process = PopenTimeout(shlex.split("siege -c %s -r %s %s -R %s/.siegerc" % (concurrency, count, url, path)), stdout = subprocess.PIPE, stderr = subprocess.STDOUT, timeout=20)
        output, _ = process.communicate()
        #Search for failed transactions
        match = re.search('Failed transactions:.*?(\d+)\n', output, re.MULTILINE)
        if match:
            trans_failures = int(match.group(1))
            print(output)
        else:
            trans_failures = concurrency * count#Failed transactions: 100%

        queries = int(cls.get_queries(config)) - queries
        rows = int(cls.get_rows(config)) - rows
        if check_updates:
            rows_updated = int(cls.get_rows_updated(config)) - rows_updated

        return queries, rows, rows_updated, cls.margin, trans_failures

    @classmethod
    def get_queries(cls, config):
        return cls.__exec_and_fetchone(config,
                                       "SELECT SUM(calls) FROM pg_stat_statements WHERE query ~* '[[:<:]]%s[[:>:]]'" % cls.tbl_name)

    @classmethod
    def get_rows(cls, config):
        return cls.__exec_and_fetchone(config,
                                       "SELECT SUM(rows) FROM pg_stat_statements WHERE query ~* '[[:<:]]%s[[:>:]]' AND query ~* 'select'" % cls.tbl_name)

    @classmethod
    def get_rows_updated(cls, config):
        return cls.__exec_and_fetchone(config,
                                       "SELECT SUM(rows) FROM pg_stat_statements WHERE query ~* '[[:<:]]%s[[:>:]]' AND query ~* 'update'" % cls.tbl_name)

    @classmethod
    def reset_cache(cls, config):
        #        To fix: DISCARD ALL cannot run inside a transaction block
        #        cursor = self.db.cursor()
        #        cursor.execute("END;DISCARD ALL;")
        #        self.db.commit()
        return

