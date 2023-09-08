import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

# Read CSV with column names
df = pd.read_csv('performance_data.csv', names=['ArraySize', 'CoreCount', 'Time'])

# Group the data by ArraySize and CoreCount and calculate the mean
grouped = df.groupby(['ArraySize', 'CoreCount']).mean().reset_index()

# Sort unique core counts
sorted_core_counts = sorted(grouped['CoreCount'].unique())

# Create a larger figure
plt.figure(figsize=(15, 10))

# Constants to scale the example curves
scale_o_n = 0.0001
scale_o_n_log_n = 0.00001

# Generate example data for O(N), O(N log N)
n_values = np.linspace(1, grouped['ArraySize'].max(), 500)
o_n = n_values * scale_o_n
o_n_log_n = n_values * np.log(n_values) * scale_o_n_log_n

plt.plot(n_values, o_n, label='O(N)', linestyle='--')
plt.plot(n_values, o_n_log_n, label='O(N log N)', linestyle='--')

# Loop through sorted unique core counts and plot averaged data
for core_count in sorted_core_counts:
    subset = grouped[grouped['CoreCount'] == core_count]
    if core_count == 1:
        plt.plot(subset['ArraySize'], subset['Time'], label=f'{core_count} Core')
    else:
        plt.plot(subset['ArraySize'], subset['Time'], label=f'{core_count} Cores')

plt.xlabel('Array Size')
plt.ylabel('Average Time (ms)')
plt.legend(loc='upper left')  # Place the legend in the upper left corner

# Set x-axis ticks to display actual numbers
xticks = np.linspace(0, grouped['ArraySize'].max(), 11)
xticklabels = [str(int(x)) for x in xticks]
plt.xticks(xticks, labels=xticklabels, rotation=45)  # Rotate for better visibility

plt.show()
