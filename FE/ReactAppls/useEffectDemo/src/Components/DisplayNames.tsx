import { DisplayName } from "./DisplayName";
import "./DisplayNames.css"

var dnCount = 0;
export const DisplayNames = () => {
    const names = ["Punith", "KGF yash"];
    dnCount++;
    
    return (
        <div className="display-names">
            count : {dnCount}
        {names.map((n, index) => (
            <div key={index}>
                
                <DisplayName name={n}></DisplayName>
            </div>
        ))}
        </div>
    )
}