import pymongo
import numpy as np
import tensorflow as tf
from tensorflow.keras.preprocessing.sequence import pad_sequences
from sklearn.model_selection import train_test_split

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
        for i in range(1, len(game_moves)):
            input_moves.append(game_moves[:i])
            output_moves.append(game_moves[i])
    return input_moves, output_moves

def encode_moves(input_moves, output_moves):
    unique_moves = list(set([move for moves in input_moves for move in moves] + output_moves))
    move_to_int = {move: idx for idx, move in enumerate(unique_moves)}
    int_input_moves = [[move_to_int[move] for move in moves] for moves in input_moves]
    int_output_moves = [move_to_int[move] for move in output_moves]
    return int_input_moves, int_output_moves, move_to_int

def preprocess_data(int_input_moves, int_output_moves, max_seq_len):
    X = pad_sequences(int_input_moves, maxlen=max_seq_len, padding='pre')
    y = np.array(int_output_moves)
    return X, y

chess_data = extract_mongodb_chess_data()
input_moves, output_moves = get_input_output_model(chess_data)
int_input_moves, int_output_moves, move_to_int = encode_moves(input_moves, output_moves)

max_sequence_length = 5
X, y = preprocess_data(int_input_moves, int_output_moves, max_sequence_length)

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

model = tf.keras.models.Sequential([
    tf.keras.layers.Embedding(input_dim=len(move_to_int), output_dim=32, input_length=max_sequence_length),
    tf.keras.layers.LSTM(units=128, return_sequences=True),
    tf.keras.layers.LSTM(units=128),
    tf.keras.layers.Dense(len(move_to_int), activation='softmax')
])

model.compile(optimizer='adam', loss='sparse_categorical_crossentropy', metrics=['accuracy'])

model.fit(X_train, y_train, epochs=5, batch_size=32, validation_data=(X_test, y_test))

model.save('chess_ai_model.keras')

# example_input = X_test[0].reshape(1, -1)  # Take one example from the test set
# predicted_move = model.predict(example_input)
# predicted_move_idx = np.argmax(predicted_move)
# predicted_move_str = list(move_to_int.keys())[list(move_to_int.values()).index(predicted_move_idx)]

# print(f"Predicted next move: {predicted_move_str}")
