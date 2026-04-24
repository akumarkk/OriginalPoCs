import { useReactTable, getCoreRowModel, flexRender, createColumnHelper } from '@tanstack/react-table';
import { z } from 'zod';

// Define and export the Zod schema for the component props
export const ComparisonTablePropsSchema = z.object({
  data: z.array(z.record(z.any())).describe('The data rows for the table.'),
  columns: z.array(z.any()).describe('The column definitions for the table.'),
});

export type ComparisonTableProps = z.infer<typeof ComparisonTablePropsSchema>;

export const ComparisonTable = ({ data, columns }: ComparisonTableProps) => {
  const table = useReactTable<any>({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });

  return (
    <div className="overflow-x-auto p-4 border rounded-lg shadow-sm">
      <table className="min-w-full text-left">
        <thead>
          {table.getHeaderGroups().map(group => (
            <tr key={group.id} className="border-b">
              {group.headers.map(header => (
                <th key={header.id} className="p-2 font-bold">
                  {flexRender(header.column.columnDef.header, header.getContext())}
                </th>
              ))}
            </tr>
          ))}
        </thead>
        <tbody>
          {table.getRowModel().rows.map(row => (
            <tr key={row.id} className="border-b hover:bg-gray-50">
              {row.getVisibleCells().map(cell => (
                <td key={cell.id} className="p-2">
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};