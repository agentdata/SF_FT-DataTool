import React, { Component } from 'react';

export class VendorTable extends Component {
  static displayName = VendorTable.name;

  constructor(props) {
    super(props);
    this.state = { foodVendors: [], loading: true };
  }

  componentDidMount() {
      this.getApprovedFoodVendors();
  }

  static renderFoodVendorsTable(foodVendors) {
    return (
      <table className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>Name</th>
            <th>Type</th>
            <th>location</th>
            <th>address</th>
            <th>permit</th>
            <th>status</th>
            <th>FoodItems</th>
          </tr>
        </thead>
        <tbody>
          {foodVendors.map(foodVendor =>
            <tr key={foodVendor.guid}>
              <td>{foodVendor.Applicant}</td>
              <td>{foodVendor.FacilityType}</td>
              <td>{foodVendor.LocationDescription}</td>
              <td>{foodVendor.Address}</td>
              <td>{foodVendor.Permit}</td>
              <td>{foodVendor.Status}</td>
              <td>{foodVendor.FoodItems}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : VendorTable.renderFoodVendorsTable(this.state.foodVendors);

    return (
      <div>
        <h1 id="tableLabel">SF Food Vendors</h1>
        <p>This page lists all food trucks in a table to be viewed.</p>
        {contents}
      </div>
    );
  }

  async getApprovedFoodVendors() {
    const response = await fetch('foodvendor/approvedvendors');
    const data = await response.json();
    
    this.setState({ foodVendors: data, loading: false });
  }
}