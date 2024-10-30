# How does A* pathfinding calculate and prioritize paths?

# What challenges arise when dynamically updating obstacles in real-time?

# How could you adapt this code for larger grids or open-world settings?

# What would your approach be if you were to add weighted cells (e.g., "difficult terrain" areas)?
Make obstacles -1, and numbers above 0 are harder terrain with a cost of the number + 1.
Maybe loop through the grid after obstacle placement and place harder terrains randomly wherever there isn't an obstacle.
