import pymongo
import numpy as np
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.sequence import pad_sequences

def extract_mongodb_chess_data():
    client = pymongo.MongoClient('mongodb://localhost:27017/')
    db = client["elite_lichess_openings_db"]
    games = db["games"]
    return list(games.find({}))

def get_unique_moves(chess_data):
    unique_moves = set()
    for game in chess_data:
        unique_moves.update(game['moves'][:8])  # Consistent with training data
    return list(unique_moves)

# Load the trained model
model = load_model('chess_ai_model.h5')

chess_data = extract_mongodb_chess_data()
unique_moves = get_unique_moves(chess_data)

move_encoder = {move: i for i, move in enumerate(unique_moves)}
move_decoder = {i: move for move, i in move_encoder.items()}

def encode_moves(moves, move_encoder):
    return [move_encoder.get(move, -1) for move in moves]

def preprocess_input(input_moves, move_encoder, max_sequence_length):
    encoded_moves = encode_moves(input_moves, move_encoder)
    padded_moves = pad_sequences([encoded_moves], maxlen=max_sequence_length)
    return padded_moves

def predict_next_move(model, input_moves, move_encoder, move_decoder, max_sequence_length):
    # Preprocess the input moves
    preprocessed_input = preprocess_input(input_moves, move_encoder, max_sequence_length)
    
    # Make a prediction
    prediction = model.predict(preprocessed_input)
    
    # Get the index of the move with the highest probability
    predicted_move_index = np.argmax(prediction)
    
    # Decode the predicted move
    predicted_move = move_decoder.get(predicted_move_index, "unknown")
    return predicted_move

# Example usage
input_moves = ["e2e4", "e7e5"]  # Example sequence of moves
max_sequence_length = 8  # Replace with the max sequence length used during training

predicted_move = predict_next_move(model, input_moves, move_encoder, move_decoder, max_sequence_length)
print(f"Predicted next move: {predicted_move}")