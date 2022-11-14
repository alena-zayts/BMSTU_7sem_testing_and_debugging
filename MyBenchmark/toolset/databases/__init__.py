import imp

db_name = 'postgres'
db = imp.load_source("Database", "/MyBenchmark/toolset/databases/postgres/postgres.py")
databases = {db_name: db.Database}

