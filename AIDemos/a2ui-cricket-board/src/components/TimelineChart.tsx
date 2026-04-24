import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { z } from 'zod';

export const TimelineChartPropsSchema = z.object({
  data: z.array(z.object({
    year: z.union([z.string(), z.number()]).describe('The label for the X-axis (e.g., year).'),
    runs: z.number().describe('The value for the Y-axis (e.g., runs).'),
  })).describe('The historical data to plot on the chart.'),
});

export type TimelineChartProps = z.infer<typeof TimelineChartPropsSchema>;

export const TimelineChart = ({ data }: TimelineChartProps) => {
  return (
    <div className="h-[300px] w-full p-4 border rounded-lg">
      <ResponsiveContainer width="100%" height="100%">
        <AreaChart data={data}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="year" />
          <YAxis />
          <Tooltip />
          <Area type="monotone" dataKey="runs" stroke="#8884d8" fill="#8884d8" />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
};