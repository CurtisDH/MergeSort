import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

# Read CSV with column names
df = pd.read_csv('performance_data.csv', names=['ArraySize', 'CoreCount', 'Time'])

# Sort unique core counts
sorted_core_counts = sorted(df['CoreCount'].unique())

# Create a larger figure
plt.figure(figsize=(15, 10))

# Initialize a flag to track if the single-core line has been plotted
single_core_plotted = False

# Loop through sorted unique core counts and plot data
for core_count in sorted_core_counts:
    subset = df[df['CoreCount'] == core_count]
    # Check if this is the single-core line and if it has already been plotted
    if core_count == 1 and single_core_plotted:
        continue
    plt.plot(subset['ArraySize'], subset['Time'], label=f'{core_count} Cores')
    if core_count == 1:
        single_core_plotted = True

# Constants to scale the example curves
scale_o_n = 0.0001
scale_o_n_log_n = 0.00001

# Generate example data for O(N), O(N log N)
n_values = np.linspace(1, df['ArraySize'].max(), 500)
o_n = n_values * scale_o_n
o_n_log_n = n_values * np.log(n_values) * scale_o_n_log_n

plt.plot(n_values, o_n, label='O(N)', linestyle='--')
plt.plot(n_values, o_n_log_n, label='O(N log N)', linestyle='--')

plt.xlabel('Array Size')
plt.ylabel('Time (ms)')
plt.legend(loc='upper left')  # Place the legend in the upper left corner

# Set x-axis ticks to display actual numbers
xticks = np.linspace(0, df['ArraySize'].max(), 11)
xticklabels = [str(int(x)) for x in xticks]
plt.xticks(xticks, labels=xticklabels, rotation=45)  # Rotate for better visibility

plt.show()
