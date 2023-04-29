import React, { useState, useEffect } from 'react'  
import Cookies from 'js-cookie';
function Dashboard() {  
    const initialUser = {FirstName: '', LastName: ''}
    const [user, setUser] = useState(initialUser)
    useEffect(() => {
        debugger; 
        console.log("rtoken",Cookies.get("rtoken"));
        var a = JSON.parse(localStorage.getItem('myData')); 
        console.log(a)           
        setUser(a)  
       
  
    }, []);  
    return (  
        <>  
            <div class="col-sm-12 btn btn-primary">  
                Dashboard  
        </div>  
            <h1>Welcome :{user.FirstName}</h1>  
        </>  
    )  
}  
  
export default Dashboard  