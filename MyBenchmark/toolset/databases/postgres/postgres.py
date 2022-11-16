import psycopg2

class Database:
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