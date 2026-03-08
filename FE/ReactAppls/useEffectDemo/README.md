# React + TypeScript + Vite

React's default behavior is:

When a parent component re-renders, ALL its children re-render too (the component function runs again)
React then checks if the output changed using its diffing algorithm
If nothing changed, the DOM doesn't get updated
In your case:

ReRenderDemo re-renders → DisplayNames re-renders → DisplayName re-renders
But since DisplayName's props (name) stay the same, the virtual DOM is identical
So the actual DOM doesn't change (which is why you don't see visual changes)


###### ReRender
Re-render = the component function runs and React creates a virtual DOM representation

This is what happens when a parent re-renders, causing all children to re-render
The component code executes again
DOM update/repaint = React actually updates the real DOM and the browser repaints the screen

This only happens if the virtual DOM differs from the previous one
This is what users actually see
Example flow:

You click button → setCount called → ReRenderDemo re-renders
ReRenderDemo function runs → DisplayNames re-renders
DisplayNames function runs → DisplayName re-renders
DisplayName function runs, returns the same JSX (props didn't change)
Re-render happened (functions executed) ✅
But DOM update didn't happen (virtual DOM same as before) ❌
So in your case:

dnCount increments (the function ran) → re-render happened
But you see the same display on screen → no DOM update
This is why optimization matters - unnecessary re-renders waste CPU cycles even if they don't update the DOM. With React.memo, you prevent step 3 entirely if props haven't changed.
