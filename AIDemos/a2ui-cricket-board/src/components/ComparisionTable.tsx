import { useReactTable, getCoreRowModel, flexRender, createColumnHelper } from '@tanstack/react-table';

export const ComparisonTable = ({ data, columns }) => {
  const table = useReactTable({
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