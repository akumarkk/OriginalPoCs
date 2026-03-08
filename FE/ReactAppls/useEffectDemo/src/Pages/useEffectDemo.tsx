import { useEffect, useState } from "react";

const userIds= [1, 2];

export const UseEffectDemo = () => {
    const [userId, setUserId] = useState(userIds[0]);
    const [isAdmin, setIsAdmin] = useState(true);

    let now = performance.now();
    while(performance.now() - now < 200) {

    }

    useEffect(() => {
        setIsAdmin(userId === userIds[0]);
    }, [userId]);

    const handleChange = () => {
        const anotherId = userIds.find(id => id !== userId)!;
        setUserId(anotherId);
    }

    return (
        <div>
            <p> userId : {userId} </p>
            <p> Admin : {isAdmin ? 'true' : 'false'} </p>
            <button title="Change User" onClick={handleChange}>Change User</button>
        </div>
    )



}