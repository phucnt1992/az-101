import { Action, Reducer } from "redux";
import { AppThunkAction } from "./";

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface HealthCheckState {
  isLoading: boolean;
  status: string;
  states: HealthCheck;
}

export interface HealthCheck {
  db: string;
  storage: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestHealthCheckAction {
  type: "REQUEST_HEALTH_CHECK";
}

interface ReceiveHealthCheckAction {
  type: "RECEIVE_HEALTH_CHECK";
  states: HealthCheck;
  status: string;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction = RequestHealthCheckAction | ReceiveHealthCheckAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
  requestHealthCheck:
    (): AppThunkAction<KnownAction> => (dispatch, getState) => {
      fetch(`health`)
        .then((response) => response.json())
        .then((data) => {
          const result = data["results"];

          dispatch({
            type: "RECEIVE_HEALTH_CHECK",
            states: {
              db: result["db"]["status"],
              storage: result["storage"]["status"],
            },
            status: data["status"],
          });
        });

      dispatch({
        type: "REQUEST_HEALTH_CHECK",
      });
    },
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: HealthCheckState = {
  states: {
    db: "",
    storage: "",
  },
  status: "",
  isLoading: false,
};

export const reducer: Reducer<HealthCheckState> = (
  state: HealthCheckState | undefined,
  incomingAction: Action
): HealthCheckState => {
  if (state === undefined) {
    return unloadedState;
  }

  const action = incomingAction as KnownAction;
  switch (action.type) {
    case "REQUEST_HEALTH_CHECK":
      return {
        states: state.states,
        status: state.status,
        isLoading: true,
      };
    case "RECEIVE_HEALTH_CHECK":
      // Only accept the incoming data if it matches the most recent request. This ensures we correctly
      // handle out-of-order responses.
      return {
        states: action.states,
        status: action.status,
        isLoading: false,
      };
    default:
      return state;
  }
};
