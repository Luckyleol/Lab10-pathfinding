# How does A* pathfinding calculate and prioritize paths?
It only checks the paths that allow it to go the farthest and closest to their desired end location, regardless of the actual length of the path. 
# What challenges arise when dynamically updating obstacles in real-time?
The calculated path from the A* algorithm could be blocked off or lead to an impossible solution where no path can be made to complete the section.
# How could you adapt this code for larger grids or open-world settings?
Generate the world via chunks to not load too many obstacles at once, similar to how minecraft works. The larger the grid, the more paths it has to check and more branches there can be.
# What would your approach be if you were to add weighted cells (e.g., "difficult terrain" areas)?
Make obstacles -1, and numbers above 0 are harder terrain with a cost of the number + 1.
Maybe loop through the grid after obstacle placement and place harder terrains randomly wherever there isn't an obstacle.
