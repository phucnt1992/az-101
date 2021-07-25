import * as React from "react";
import { connect } from "react-redux";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../store";
import * as HealthCheckStore from "../store/HealthCheck";

// At runtime, Redux will merge together...
type HealthCheckProps = HealthCheckStore.HealthCheckState & // ... state we've requested from the Redux store
  typeof HealthCheckStore.actionCreators & // ... plus action creators we've requested
  RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters

class HealthCheck extends React.PureComponent<HealthCheckProps> {
  // This method is called when the component is first added to the document
  public componentDidMount() {
    this.props.requestHealthCheck();
  }

  public render() {
    return (
      <React.Fragment>
        <h1 id="tabelLabel">Health Check</h1>
        <p>This component demonstrates health checking dependency services.</p>
        {this.renderData()}
      </React.Fragment>
    );
  }

  private renderData() {
    const { isLoading, states, status } = this.props;

    if (isLoading)
      return (
        <div className="d-flex justify-content-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      );

    return (
      <>
        <table className="table table-striped" aria-labelledby="tabelLabel">
          <thead>
            <tr>
              <th>Service</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr key={"db"}>
              <td>SQL Server</td>
              <td>{states.db}</td>
            </tr>
            <tr key={"storage"}>
              <td>Storage</td>
              <td>{states.storage}</td>
            </tr>
          </tbody>
        </table>
        <p>
          <strong>System Status:</strong> {status}
        </p>
      </>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.healthCheck, // Selects which state properties are merged into the component's props
  HealthCheckStore.actionCreators // Selects which action creators are merged into the component's props
)(HealthCheck as any);
