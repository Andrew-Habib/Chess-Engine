import chess.pgn as pgnLibrary
import pymongo

def pgn_to_mongo(filenames, games):
    for filename in filenames:
        with open(filename) as file:
            while True:
                # Read each game in the pgn file
                game = pgnLibrary.read_game(file)
                if game is None:
                    break
                # Extract the first 8 moves (or less) from each game
                num_moves = 0
                moves = []
                for move in game.mainline_moves():
                    moves.append(str(move))
                    num_moves = num_moves + 1
                    if num_moves == 8:
                        break
                # Insert each game into MongoDB
                games.insert_one({
                    "game_id": game.headers["LichessURL"],
                    "white": game.headers["White"],
                    "black": game.headers["Black"],
                    "moves": moves,
                    "result": game.headers["Result"]
                })

filenames = ['lichess_elite_2022-03.pgn']

client = pymongo.MongoClient('mongodb://localhost:27017/')
client.drop_database("elite_lichess_openings_db")
db = client["elite_lichess_openings_db"]
games = db["games"]

pgn_to_mongo(filenames=filenames, games=games)