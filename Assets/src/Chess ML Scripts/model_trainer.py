import pymongo
import numpy as np
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Embedding, LSTM, Dense


def extract_mongodb_chess_data():
    client = pymongo.MongoClient('mongodb://localhost:27017/')
    db = client["elite_lichess_openings_db"]
    games = db["games"]
    return list(games.find({}))



def get_input_output_model(chess_data):
    input_moves = []
    output_moves = []
    for game in chess_data:
        game_moves = game['moves']
        for i in range(0, len(game_moves)):
            input_moves.append(game_moves[:i])
            output_moves.append(game_moves[i])
    return input_moves, output_moves

chess_data = extract_mongodb_chess_data()
X, y = get_input_output_model(chess_data=chess_data)

print(X, y)