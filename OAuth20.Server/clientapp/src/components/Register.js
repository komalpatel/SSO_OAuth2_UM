import React, { useState } from 'react'  
import axios from 'axios';  
import { useNavigate } from 'react-router-dom';

function Register(props) {  
  const [data, setdata] = useState({ Email: '', Password: '', FirstName: '', LastName: '', UserName: '', PhoneNumber: ''  })  
  const apiUrl = "https://localhost:7275/accounts/register";  
  const navigate = useNavigate();

  const Registration = (e) => {  
    e.preventDefault();  
    const data1 = { Email: data.Email, Password: data.Password, FirstName: data.FirstName, LastName: data.LastName, UserName: data.UserName, PhoneNumber: data.PhoneNumber };  
    axios.post(apiUrl, data1)  
      .then((result) => {  
        debugger;  
        console.log("Register:",result.data);  
        if (result.data.Status === 'Invalid')  
          alert('Invalid User');  
        else  
        navigate('/Dashboard', { replace: true })   
      })  
  }  
  const onChange = (e) => {  
    e.persist();  
    debugger;  
    setdata({ ...data, [e.target.name]: e.target.value });  
  }  
  return (  
    <div class="container"> 
    <div class="card-deck">
    
        <div class="card m-3">
                <h5 class="card-header text-white">Register</h5>
            <div class="card-body">
                <form onSubmit={Registration} class="user">  
                <p>Please fill in this form to create an account.</p>
                <div class="form-group"> 
                    <label for="FirstName">First Name</label>
                      <input type="text" name="FirstName" onChange={onChange} value={data.FirstName} class="form-control" id="exampleFirstName" placeholder="First Name" />  
                    </div>  
                    <div class="form-group">
                    <label for="LastName">Last Name</label>
                      <input type="text" name="LastName" onChange={onChange} value={data.LastName} class="form-control" id="exampleLastName" placeholder="Last Name" />  
                    </div>  
                    <div class="form-group">
                    <label for="exampleDropdownFormEmail1">User Name</label>
                      <input type="text" name="UserName" onChange={onChange} value={data.UserName} class="form-control" id="exampleUserName" placeholder="User Name" />  
                    </div>  
                    <div class="form-group"> 
                    <label for="exampleDropdownFormEmail1">Email address</label>
                      <input type="text" name="Email" onChange={onChange} value={data.Email} class="form-control" id="exampleEmailAddress" placeholder="Email Address" />  
                    </div> 
                    <div class="form-group">
                    <label for="phone">Mobile number</label>
                        <input type="text" name="PhoneNumber" onChange={onChange} value={data.PhoneNumber} class="form-control" placeholder="00249" id="phone"/>
                    </div> 
                    <div class="form-group"> 
                    <label for="exampleDropdownFormPassword1">Password</label>
                      <input type="Password" name="Password" onChange={onChange} value={data.Password} class="form-control" id="exampleLastName" placeholder="Password" />  
                    </div>  
                    <div class="form-group">
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input" id="dropdownCheck"/>
                            <label class="form-check-label" for="dropdownCheck">
                                By creating an account you agree to our Terms & Privacy.
                            </label>
                        </div>
                    </div>
                  <button type="submit" class="btn btn-primary  btn-block">  
                    Sign up  
                </button>  
                 
                </form>  
                
              </div>  
            </div>  
          </div>  
        </div>  
  
  )  
}  
  
export default Register  