import React, { useState } from 'react'   
import axios from 'axios';  
import {Link,  useNavigate } from 'react-router-dom';
import jwt_decode from "jwt-decode";
import { BASE_URL } from './services/Settings';
import SessionManager from './Auth/SessionManager'; 
import Cookies from 'js-cookie';

function Login(props) {  
    const [LoginRequest, setemployee] = useState({ UserName: '', Password: ''});  

    const navigate = useNavigate();
 
    // var token =  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFsZXhAY2hhcmdlYXRmcmllbmRzLmNvbSIsImdpdmVuX25hbWUiOiJhbGV4QGNoYXJnZWF0ZnJpZW5kcy5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiI3MmNlNDk0Yy01YmY0LTRmNzMtYmJkZi1hZTBiN2RhZDA0YWIiLCJhdWQiOlsiY2hhcmdlYXRmcmllbmRzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NzI3NSJdLCJqdGkiOiI3MmNlNDk0Yy01YmY0LTRmNzMtYmJkZi1hZTBiN2RhZDA0YWIiLCJuYmYiOjE2ODEzNzY3MTUsImV4cCI6MTY5NDU5NTkxNSwiaWF0IjoxNjgxMzc2NzE1LCJpc3MiOiJjaGFyZ2VhdGZyaWVuZHMifQ.5_mnV6v9cQbp5SSYueG-SP9hts5TwrJzD1W9wwbmTdQ";
    // var decoded = jwt_decode(token);

    const Login = (e) => {    
            e.preventDefault();    
            debugger;   
            const data = { UserName:LoginRequest.UserName, Password: LoginRequest.Password };   
            axios.post(BASE_URL+'accounts/CAFLogin', data)    
            .then((result) => {    
                console.log("Result:",result);
                const serializedState = JSON.stringify(result.data.UserDetails);  
                var a= localStorage.setItem('myData', serializedState);   
                console.log("A:",a) 
                SessionManager.setUserSession(result.data.UserName, result.data.Token, result.data.Email,result.data.GuID)
              
                localStorage.setItem('rtoken', result.data.Token);

                console.log("rtoken",localStorage.getItem("rtoken"));
                if (result.status === 200)    
                {
                  console.log("rtoken",Cookies.get("rtoken"));
                  navigate('/Dashboard', { replace: true })   
                }                    
                else    
                alert('Invalid User');    
            })        
          };    
          
          const onChange = (e) => {    
                e.persist();    
                setemployee({...LoginRequest, [e.target.name]: e.target.value});    
              }    
    return (  
        
      

        
<div class="container"> 
<div class="card-deck">

    <div class="card m-3">
            <h5 class="card-header text-white">Login</h5>
        <div class="card-body">
        <form onSubmit={Login} class="user">  
                        <div class="form-group">  
                          <input type="UserName" class="form-control" value={LoginRequest.UserName} onChange={ onChange }  name="UserName" id="UserName" aria-describedby="emailHelp" placeholder="Enter Email"/>  
                        </div>  
                        <div class="form-group">  
                          <input type="password" class="form-control" value={LoginRequest.Password} onChange={ onChange }  name="Password" id="DepPasswordartment" placeholder="Password"/>  
                        </div>  
                        <div class="form-group">  
                          <div class="custom-control custom-checkbox small">  
                            <input type="checkbox" class="custom-control-input" id="customCheck"/>  
                            <label class="custom-control-label" for="customCheck">Remember Me</label>  
                          </div>  
                        </div>
                        <button type="submit" class="btn btn-primary"  block><span>Login</span></button>    
                         
          </form>  
          <div class="form-group">
                   
           
                   </div>
          <div class="dropdown-divider"></div>
          <Link className='text-red-500' to="/recovery">Forgot Password? </Link>
          </div>
      </div>
</div>
</div>  
    )  
}  
  
export default Login