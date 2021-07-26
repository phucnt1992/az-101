import { Action, Reducer } from "redux";
import { AppThunkAction } from ".";

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface PhotoState {
  isLoading: boolean;
  isSubmitting: boolean;
  data: Photo[];
}

export interface Photo {
  id: number;
  title: string;
  alt: string;
  createdAt: string;
  updatedAt: string;
  fileUrl: string;
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.

interface RequestPhotoAction {
  type: "REQUEST_PHOTO";
}

interface ReceivePhotoAction {
  type: "RECEIVE_PHOTO";
  data: Photo[];
}

interface CreatePhotoAction {
  type: "CREATE_PHOTO" | "CREATE_PHOTO_SUCCESS";
}

interface DeletePhotoAction {
  type: "DELETE_PHOTO" | "DELETE_PHOTO_SUCCESS";
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
type KnownAction =
  | RequestPhotoAction
  | ReceivePhotoAction
  | CreatePhotoAction
  | DeletePhotoAction;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
  requestPhotos: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
    fetch(`api/photos`)
      .then((response) => response.json() as Promise<Photo[]>)
      .then((data) => {
        dispatch({
          type: "RECEIVE_PHOTO",
          data,
        });
      });

    dispatch({
      type: "REQUEST_PHOTO",
    });
  },

  createPhoto:
    (formData: FormData): AppThunkAction<KnownAction> =>
    (dispatch, getState) => {
      fetch(`api/photos`, {
        method: "POST",
        body: formData,
      }).then(() =>
        dispatch({
          type: "CREATE_PHOTO_SUCCESS",
        })
      );

      dispatch({
        type: "CREATE_PHOTO",
      });
    },

  deletePhoto:
    (id: number): AppThunkAction<KnownAction> =>
    (dispatch, getState) => {
      fetch(`api/photos/${id}`, {
        method: "DELETE",
      }).then(() => dispatch({ type: "DELETE_PHOTO_SUCCESS" }));

      dispatch({
        type: "DELETE_PHOTO",
      });
    },
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

const unloadedState: PhotoState = {
  data: [],
  isLoading: false,
  isSubmitting: false,
};

export const reducer: Reducer<PhotoState> = (
  state: PhotoState | undefined,
  incomingAction: Action
): PhotoState => {
  if (state === undefined) {
    return unloadedState;
  }

  const action = incomingAction as KnownAction;
  switch (action.type) {
    case "REQUEST_PHOTO":
      return {
        ...state,
        data: state.data,
        isLoading: true,
      };
    case "RECEIVE_PHOTO":
      // Only accept the incoming data if it matches the most recent request. This ensures we correctly
      // handle out-of-order responses.
      return {
        ...state,
        data: action.data,
        isLoading: false,
      };
    case "CREATE_PHOTO":
    case "DELETE_PHOTO":
      return {
        ...state,
        isSubmitting: true,
      };
    case "CREATE_PHOTO_SUCCESS":
    case "DELETE_PHOTO_SUCCESS":
      return {
        ...state,
        isSubmitting: false,
      };
    default:
      return state;
  }
};
