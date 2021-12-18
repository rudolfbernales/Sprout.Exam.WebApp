import React, { Component } from 'react';
import authService from '../../components/api-authorization/AuthorizeService';

export class EmployeeEdit extends Component {
  static displayName = EmployeeEdit.name;

  constructor(props) {
    super(props);
    this.state = { id: 0,fullName: '',birthdate: '',tin: '',typeId: 1, rate: 0, loading: true,loadingSave:false };
  }

  componentDidMount() {
    this.getEmployee(this.props.match.params.id);
  }
  handleChange(event) {
    this.setState({ [event.target.name] : event.target.value});
  }

  handleSubmit(e){
      e.preventDefault();
      if (window.confirm("Are you sure you want to save?")) {
        this.saveEmployee();
      } 
  }

  render() {

    let contents = this.state.loading
    ? <p><em>Loading...</em></p>
    : <div>
    <form>
<div className='form-row'>
<div className='form-group col-md-6'>
  <label htmlFor='inputFullName4'>Full Name: *</label>
  <input type='text' className='form-control' id='inputFullName4' onChange={this.handleChange.bind(this)} name="fullName" value={this.state.fullName} placeholder='Full Name' />
</div>
<div className='form-group col-md-6'>
  <label htmlFor='inputBirthdate4'>Birthdate: *</label>
  <input type='date' className='form-control' id='inputBirthdate4' onChange={this.handleChange.bind(this)} name="birthdate" value={this.state.birthdate} placeholder='Birthdate' />
</div>
</div>
<div className="form-row">
<div className='form-group col-md-6'>
  <label htmlFor='inputTin4'>TIN: *</label>
  <input type='text' className='form-control' id='inputTin4' onChange={this.handleChange.bind(this)} value={this.state.tin} name="tin" placeholder='TIN' />
</div>
<div className='form-group col-md-6'>
  <label htmlFor='inputEmployeeType4'>Employee Type: *</label>
  <select id='inputEmployeeType4' onChange={this.handleChange.bind(this)} value={this.state.typeId}  name="typeId" className='form-control'>
    <option value='1'>Regular</option>
    <option value='2'>Contractual</option>
  </select>
</div>
{ this.state.typeId?.toString() === '1'  &&
  <div className='form-group col-md-6'>
  <label htmlFor='inputRate4'>Monthly Rate: *</label>
  <input type='text' className='form-control' id='inputRate4' onChange={this.handleChange.bind(this)} value={this.state.rate} name="rate" placeholder='Rate' />
  </div>
}
{ this.state.typeId?.toString() === '2' &&
  <div className='form-group col-md-6'>
  <label htmlFor='inputRate4'>Rate Per Day: *</label>
  <input type='text' className='form-control' id='inputRate4' onChange={this.handleChange.bind(this)} value={this.state.rate} name="rate" placeholder='Rate' />
  </div>
}
</div>
<button type="submit" onClick={this.handleSubmit.bind(this)} disabled={this.state.loadingSave} className="btn btn-primary mr-2">{this.state.loadingSave?"Loading...": "Save"}</button>
<button type="button" onClick={() => this.props.history.push("/employees/index")} className="btn btn-primary">Back</button>
</form>
</div>;


    return (
        <div>
        <h1 id="tabelLabel" >Employee Edit</h1>
        <p>All fields are required</p>
        {contents}
      </div>
    );
  }

  async saveEmployee() {

    if(!this.validateForm()){
      this.setState({ loadingSave: true });
      const token = await authService.getAccessToken();
      const requestOptions = {
          method: 'PUT',
          headers: !token ? {} : { 'Authorization': `Bearer ${token}`,'Content-Type': 'application/json' },
          body: JSON.stringify(this.state)
      };
      const response = await fetch('api/employees/' + this.state.id,requestOptions);
  
      if(response.status === 200){
          this.setState({ loadingSave: false });
          alert("Employee successfully saved");
          this.props.history.push("/employees/index");
      }
      else{
          alert("There was an error occured.");
      }
    }
    else{
      alert('Please check your input and try again...');
    }
   
  }

  async getEmployee(id) {
    this.setState({ loading: true,loadingSave: false });
    const token = await authService.getAccessToken();
    const response = await fetch('api/employees/' + id, {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });

    if(response.status === 204){
      alert('Employee Does Not Exist..');
      this.props.history.push("/employees/index");
    }
    const data = await response.json();
    this.setState({ id: data.id,fullName: data.fullName, birthdate: this.formatDate(data.birthdate), tin: data.tin,typeId: data.typeId, rate: data.rate, loading: false,loadingSave: false });
  }

   formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) 
        month = '0' + month;
    if (day.length < 2) 
        day = '0' + day;

    return [year, month, day].join('-');
  }

  validateForm(){
    let formValues = this.state;
    let isInvalid = false;
    const keys = Object.keys(formValues);
    for(let i = 0; i < keys.length; i++){
        if(formValues[keys[i]] === ''){
          isInvalid = true;
          break;
        }
        else if(formValues[keys[i]] === 0 && keys[i] === 'rate'){
          isInvalid = true;
          break;
        }
    }
    return isInvalid;
  }

}
