### Server Responsibilities (High-Level Roadmap)

#### 1. Canonical State Management
- Maintain full authoritative world state (grid, buildings, resources)
- Store all data in world-space coordinates
- Track player metadata, including perspective (e.g., top vs bottom)

#### 2. Input Handling
- Receive client commands with local-space data
- Transform to world-space using sender’s perspective
- Validate action (position, rules, ownership)
- Apply to world state if valid

#### 3. Perspective-Aware Broadcasting
- For each connected client:
    - Transform updated world state into client-local perspective
    - Send transformed state delta (not full state)
    - Do **not** send to the originator unless necessary (e.g., confirmations)

#### 4. Sync Guarantees
- Ensure all clients display the same game state from their own perspective
- Keep animation direction, particles, and visuals consistent via transformation rules

#### 5. Modular Transform System
- Centralize all flip/mirror logic in a transform service
- Apply to positions, directions, animation data before sending to clients


## TODO:
General:
  - Use dependency container to add grid instance (to many grid injections everywhere). (Use Michael's implementation)