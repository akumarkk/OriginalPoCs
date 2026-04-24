// import { createCatalog } from '@a2ui/react';
import { createCatalog } from '@copilotkit/a2ui-renderer';
import { ComparisonTable, ComparisonTablePropsSchema } from './components/ComparisonTable'; // Wrapper for TanStack
import { TimelineChart, TimelineChartPropsSchema } from './components/TimelineChart';     // Wrapper for Recharts
import { FilterableStatsGrid, FilterableStatsGridPropsSchema } from './components/FilterableStatsGrid';             // Custom Flexbox/Grid
import { DeepDiveArticle, DeepDiveArticlePropsSchema } from './components/DeepDiveArticle';               // Typography/Article

export const Catalog = createCatalog({
  // The 'key' (left) is what your Agent requests in JSON
  // The 'value' (right) is your React component, often with a Zod schema for props
  'ComparisonTable': {
    component: ComparisonTable,
    props: ComparisonTablePropsSchema as any,
  },
  'TimelineChart': {
    component: TimelineChart,
    props: TimelineChartPropsSchema as any,
  },
  'FilterableStatsGrid': {
    component: FilterableStatsGrid,
    props: FilterableStatsGridPropsSchema as any,
  },
  'DeepDiveArticle': {
    component: DeepDiveArticle,
    props: DeepDiveArticlePropsSchema as any,
  },
}, {});