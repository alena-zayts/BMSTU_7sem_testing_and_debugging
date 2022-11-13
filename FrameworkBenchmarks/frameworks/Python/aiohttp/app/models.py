from sqlalchemy import Column, Integer, String
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

class World(Base):
    __tablename__ = 'world'
    id = Column(Integer, primary_key=True)
    randomnumber = Column(Integer)

sa_worlds = World.__table__

