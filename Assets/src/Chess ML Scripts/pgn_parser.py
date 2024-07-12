import chess.pgn as pgnLibrary
import pymongo

def png_to_mongo(filename, games):
    with open(filename) as file:
        game = ""
        while game is not None:
            game = pgnLibrary.read_game(file)
            print(game)

filename = 'lichess_elite_2022-03.pgn'

client = pymongo.MongoClient('localhost:27017')
db = client["lichess_db"]
games = db["games"]

png_to_mongo(filename=filename, games=games)