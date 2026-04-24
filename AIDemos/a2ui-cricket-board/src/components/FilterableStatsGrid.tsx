import { useState } from 'react';
import { Search } from 'lucide-react'; // Great for icons
import { z } from 'zod';

export const FilterableStatsGridPropsSchema = z.object({
  stats: z.array(z.object({
    label: z.string().describe('The name of the statistic.'),
    value: z.union([z.string(), z.number()]).describe('The value of the statistic.'),
  })).describe('The statistics to display.'),
  title: z.string().optional().describe('The header title for the grid.'),
});

export type FilterableStatsProps = z.infer<typeof FilterableStatsGridPropsSchema>;

export const FilterableStatsGrid = ({ stats, title = "Statistics" }: FilterableStatsProps) => {
  const [searchTerm, setSearchTerm] = useState("");

  // Logic: Filter based on label matches
  const filteredStats = stats.filter((stat) =>
    stat.label.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="w-full bg-white p-6 rounded-xl border border-gray-200 shadow-sm">
      {/* Header & Search Input */}
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl font-bold text-gray-800">{title}</h2>
        <div className="relative">
          <Search className="absolute left-3 top-2.5 text-gray-400 w-4 h-4" />
          <input
            type="text"
            placeholder="Search stats..."
            className="pl-10 pr-4 py-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 outline-none"
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
      </div>

      {/* Grid Layout */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {filteredStats.map((stat, index) => (
          <div key={index} className="p-4 border border-gray-100 rounded-lg bg-gray-50 hover:bg-blue-50 transition-colors">
            <p className="text-xs text-gray-500 uppercase font-semibold">{stat.label}</p>
            <p className="text-xl font-bold text-gray-900 mt-1">{stat.value}</p>
          </div>
        ))}
      </div>
      
      {filteredStats.length === 0 && (
        <p className="text-center text-gray-400 py-4">No statistics found matching "{searchTerm}"</p>
      )}
    </div>
  );
};