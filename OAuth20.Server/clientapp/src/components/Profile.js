import React, { useState } from 'react'  
import axios from 'axios';  
import { useNavigate } from 'react-router-dom';
import { BASE_URL } from './services/Settings';

function Register(props) {  
  const [data, setdata] = useState({ FirstName: '', LastName: '', UserName: '', PhoneNumber: ''  })  
  const navigate = useNavigate();

  const changeprofile = (e) => {  
    e.preventDefault();  
    const data1 = { Password: data.Password, FirstName: data.FirstName, LastName: data.LastName, UserName: data.UserName, PhoneNumber: data.PhoneNumber };  
    
    
    axios.get(BASE_URL + 'Manage/Changeprofile').then((response) => {
      console.log("Get:",response);
        setdata(response.data);
        console.log("Profile:",response.data);  
      });

    //   React.useEffect(() => {
    //         setdata(response.data);
    //     });     axios.get(apiUrl).then((response) => {
   
    //   }, []);
    

    axios.post(BASE_URL + 'Manage/Changeprofile', data1)  
      .then((result) => {  
        debugger;  
        console.log("Profile:",result.data);  
        if (result.data.Status === 'Invalid')  
          alert('Invalid User');  
        else  
        navigate('/Dashboard', { replace: true })   
      })  
  }  
  const onChange = (e) => {  
    e.persist();  
    //debugger;  
    setdata({ ...data, [e.target.name]: e.target.value });  
  }  
  return (  
    <div class="container"> 
    <div class="card-deck">
    
        <div class="card m-3">
                <h5 class="card-header text-white">Profile</h5>
            <div class="card-body">
                <form onSubmit={changeprofile} class="user">  
             
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
                    <label for="phone">Mobile number</label>
                        <input type="text" name="PhoneNumber" onChange={onChange} value={data.PhoneNumber} class="form-control" placeholder="00249" id="phone"/>
                    </div> 
              
                  <button type="submit" class="btn btn-primary  btn-block">  
                    Submit
                </button>  
                 
                </form>  
                
              </div>  
            </div>  
          </div>  
        </div>  
  
  )  
}  
  
export default Register  