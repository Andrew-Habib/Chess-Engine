import chess
import chess.engine

def generate_positions(board, depth):
    if depth == 0:
        return [board.fen()]
    
    positions = []
    for move in board.legal_moves:
        board.push(move)
        positions.extend(generate_positions(board, depth - 1))
        board.pop()
    return positions

def main():
    board = chess.Board()
    depth = 4
    all_positions = generate_positions(board, depth)
    
    # Print or save all positions
    print(f"Total positions at depth {depth}: {len(all_positions)}")

main()