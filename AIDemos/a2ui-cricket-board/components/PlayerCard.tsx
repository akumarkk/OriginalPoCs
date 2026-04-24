import { A2UISurface, createCatalog } from '@a2ui/react';

// Create a reusable component for your legends
const PlayerCard = ({ name, nickname, stats, achievement }: any) => (
  <div style={{ border: '1px solid #ccc', padding: '1rem', borderRadius: '8px', margin: '10px' }}>
    <h2>{name}</h2>
    <p><em>"{nickname}"</em></p>
    <ul>
      {stats.map((stat: string, i: number) => <li key={i}>{stat}</li>)}
    </ul>
    <div style={{ background: '#f0f0f0', padding: '5px' }}>
      <strong>Career Highlight:</strong> {achievement}
    </div>
  </div>
);

// Register it in your catalog
const catalog = createCatalog({
  'PlayerCard': PlayerCard,
});