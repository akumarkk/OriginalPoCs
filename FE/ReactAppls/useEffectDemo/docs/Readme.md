###### Rerender
Nested comp gets rerendered even if the outer memo component isn't rerendering/causing nested comp to rerender;


- state
- context : If your nested components are consuming data from a React Context (using the useContext hook), they will re-render whenever the Context Provider's value changes.
- global state