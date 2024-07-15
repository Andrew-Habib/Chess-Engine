import pymongo
import numpy
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

def get_num_unique_moves(chess_data):
    np_chess_data = numpy.array(chess_data)
    unique_moves = set()
    for data in np_chess_data:
        unique_moves.update(data)
    return len(unique_moves)

chess_data = extract_mongodb_chess_data()
X, y = get_input_output_model(chess_data=chess_data)

num_unique_moves = get_num_unique_moves(chess_data=chess_data)
embedding_dim = 100
num_epochs = 10

model = Sequential() # Defining the neural network (Deep learning model)
model.add(Embedding(input_dim=num_unique_moves, output_dim=embedding_dim))
model.add(LSTM(units=128))
model.add(Dense(units=num_unique_moves, activation='softmax'))

model.compile(optimizer='adam', loss='sparse_categorical_crossentropy', metrics=['accuracy'])

model.fit(X, y, epochs=num_epochs, validation_split=0.2)

model.save('chess_ai_model.h5')
