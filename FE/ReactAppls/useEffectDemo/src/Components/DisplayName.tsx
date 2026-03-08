
import "./DisplayName.css"

var dnCount = 0;
export const DisplayName = ({name}) => {
    // let dnCount = 0;
    dnCount++;
    return (
        <div className="display-name">
        <p>
            dnCount : {dnCount}
            Name is {name};
        </p>
        </div>
    )
}