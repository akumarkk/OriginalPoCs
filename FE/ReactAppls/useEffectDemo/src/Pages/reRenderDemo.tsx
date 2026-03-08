import { useState } from "react";
import { DisplayNames } from "../Components/DisplayNames";

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
        </>

    )
}