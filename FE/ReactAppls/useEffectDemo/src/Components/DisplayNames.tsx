import { DisplayName } from "./DisplayName";
var dnCount = 0;
export const DisplayNames = () => {
    const names = ["Punith", "KGF yash"];
    dnCount++;
    
    return (
        <>
        {names.map((n, index) => (
            <div key={index}>
                count : {index}/{dnCount}
                <DisplayName name={n}></DisplayName>
            </div>
        ))}
        </>
    )
}