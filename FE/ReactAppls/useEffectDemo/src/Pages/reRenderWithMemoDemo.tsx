import { useMemo, useState } from "react";
import { DisplayNames } from "../Components/DisplayNames";
import { DisplayNamesMemo } from "../Components/DisplayNamesMemo";

function factorial(n) {

    let i=0;
    while(i < 200000000)
    {
        i++;
    }

    if(n< 0){
        return -1;

    }

    if(n===0)
    {
        return 1;
    }

    return n* factorial(n-1);
}

export const ReRenderWithMemoDemo = () => {
    const [count, setCount] = useState(0);
    const [name, setName] =useState("0");

    let result = useMemo(() => {
        console.log(`rerendered factorial`);
        return factorial(count);
    }, [count]); 

    const handleClick = () => {
        setCount(count+1);
    }

    const handleChange = (e) => {
        setName(e.target.value);
    }

    return(
        <>
            <p>
                Factorial of {count} : {result}
            </p>
            
            <button onClick={handleClick}>Increment</button>
            <div>
                <div>
                    <label htmlFor="name">Enter name</label>
                </div>
                <div>
                    <input type="text" placeholder="enter name..." onChange={handleChange}/>
                    <p>Name is {name}</p>
                </div>
            </div>

            {/* <DisplayNames></DisplayNames>

            <h2>
                No rerender on Increment
            </h2>
            <DisplayNamesMemo /> */}
        </>

    )
}