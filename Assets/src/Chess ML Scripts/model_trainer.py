import pymongo
import numpy as np
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Embedding, LSTM, Dense, Dropout
from tensorflow.keras.preprocessing.sequence import pad_sequences
from tensorflow.keras.utils import to_categorical
from tensorflow.keras.callbacks import EarlyStopping, ModelCheckpoint

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

def get_unique_moves(chess_data):
    unique_moves = set()
    for game in chess_data:
        unique_moves.update(game['moves'])
    return list(unique_moves)

def encode_moves(moves, move_encoder):
    return [move_encoder[move] for move in moves]

chess_data = extract_mongodb_chess_data()
input_moves, output_moves = get_input_output_model(chess_data)
unique_moves = get_unique_moves(chess_data)

move_encoder = {move: i for i, move in enumerate(unique_moves)}
encoded_input_moves = [encode_moves(moves, move_encoder) for moves in input_moves]
encoded_output_moves = encode_moves(output_moves, move_encoder)


max_sequence_length = max(len(moves) for moves in encoded_input_moves)
embedding_dim = 100
num_epochs = 20  # Increase the number of epochs
batch_size = 32  # Use a smaller batch size

X = pad_sequences(encoded_input_moves, maxlen=max_sequence_length)
y = to_categorical(encoded_output_moves, num_classes=len(unique_moves))

model = Sequential()
model.add(Embedding(input_dim=len(unique_moves), output_dim=embedding_dim, input_length=max_sequence_length))
model.add(LSTM(units=128, return_sequences=True))
model.add(Dropout(0.2))  # Add dropout for regularization
model.add(LSTM(units=128))
model.add(Dropout(0.2))  # Add dropout for regularization
model.add(Dense(units=len(unique_moves), activation='softmax'))

model.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy'])

# Use early stopping and model checkpointing for better performance
early_stopping = EarlyStopping(monitor='val_loss', patience=3, restore_best_weights=True)
model_checkpoint = ModelCheckpoint('best_chess_ai_model.keras', save_best_only=True)

model.fit(X, y, epochs=num_epochs, batch_size=batch_size, validation_split=0.2, callbacks=[early_stopping, model_checkpoint])

model.save('chess_ai_model.keras')