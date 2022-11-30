import React, { Component } from 'react';

export class Analytics extends Component {
  static displayName = Analytics.name;

  constructor(props) {
    super(props);
    this.state = { foodVendors: [], loading: true };
  }

  componentDidMount() {
    this.populateFoodVendors();
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
            <tr key={foodVendor.applicant}>
              <td>{foodVendor.facilityType}</td>
              <td>{foodVendor.locationDescription}</td>
              <td>{foodVendor.address}</td>
              <td>{foodVendor.permit}</td>
              <td>{foodVendor.status}</td>
              <td>{foodVendor.foodItems}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Analytics.renderFoodVendorsTable(this.state.foodVendors);

    return (
      <div>
        <h1 id="tableLabel">SF Food Vendors</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async populateFoodVendors() {
    const response = await fetch('foodvendors');
    const data = await response.json();
    this.setState({ foodVendors: data, loading: false });
  }
}