import { createCatalog } from '@a2ui/react';
import { ComparisonTable } from './components/ComparisonTable'; // Wrapper for TanStack
import { TimelineChart } from './components/TimelineChart';     // Wrapper for Recharts
import { StatsGrid } from './components/StatsGrid';             // Custom Flexbox/Grid
import { DeepDive } from './components/DeepDive';               // Typography/Article

export const myCatalog = createCatalog({
  // The 'key' (left) is what your Agent requests in JSON
  // The 'value' (right) is your React component
  'ComparisonTable': ComparisonTable,
  'TimelineChart': TimelineChart,
  'FilterableStatsGrid': StatsGrid,
  'DeepDiveArticle': DeepDive,
});