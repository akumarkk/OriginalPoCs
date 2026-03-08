import { useState } from "react";
import { DisplayNames } from "../Components/DisplayNames";
import { DisplayNamesMemo } from "../Components/DisplayNamesMemo";

export const ReRenderDemo = () => {
    const [count, setCount] = useState(0);

    const handleClick = () => {
        setCount(count+1);
    }

    return(
        <>
            reRender Counter: { count }
            <button onClick={handleClick}>Increment</button>

            <DisplayNames></DisplayNames>

            <h2>
                No rerender on Increment
            </h2>
            <DisplayNamesMemo />
        </>

    )
}