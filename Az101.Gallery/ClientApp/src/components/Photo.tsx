import * as React from "react";
import moment from "moment";
import { connect } from "react-redux";
import { RouteComponentProps } from "react-router";
import { ApplicationState } from "../store";
import * as PhotosStore from "../store/Photo";

// At runtime, Redux will merge together...
type PhotoProps = PhotosStore.PhotoState & // ... state we've requested from the Redux store
  typeof PhotosStore.actionCreators & // ... plus action creators we've requested
  RouteComponentProps<{ startDateIndex: string }>; // ... plus incoming routing parameters

class Photo extends React.PureComponent<PhotoProps> {
  // This method is called when the component is first added to the document
  formRef: any;

  constructor(props: PhotoProps) {
    super(props);
    this.formRef = React.createRef();
  }
  public componentDidMount() {
    this.props.requestPhotos();
  }

  componentDidUpdate(prevProps: PhotoProps) {
    if (
      prevProps.isSubmitting === true &&
      prevProps.isSubmitting !== this.props.isSubmitting
    ) {
      this.formRef.current.reset();
      this.props.requestPhotos();
    }
  }

  public render() {
    return (
      <React.Fragment>
        <h1 id="tabelLabel">Photo Gallery</h1>
        <p>
          This component demonstrates integrating SQL Server and Azure Storage
          feature.
        </p>
        {this.renderForm()}
        <hr />
        {this.renderData()}
      </React.Fragment>
    );
  }

  private onSubmit(e: React.SyntheticEvent) {
    e.preventDefault();

    const target = e.target as typeof e.target & {
      title: { value: string };
      alt: { value: string };
      formFile: { files: FileList };
    };

    const formData = new FormData();
    formData.append("title", target.title.value);
    formData.append("alt", target.alt.value);
    formData.append("file", target.formFile.files[0]);

    this.props.createPhoto(formData);
  }

  private renderData() {
    const { data, isLoading, deletePhoto } = this.props;

    if (isLoading)
      return (
        <div className="d-flex justify-content-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      );

    return (
      <div className="row row-cols-1 row-cols-md-3 g-4">
        {data.map((photo: PhotosStore.Photo) => (
          <div className="col" key={photo.id}>
            <div className="card h-100">
              <img
                src={photo.fileUrl}
                title={photo.title}
                className="card-img-top"
                alt={photo.alt}
              />
              <div className="card-body">
                <h5 className="card-title">{photo.title}</h5>
                <button
                  className="btn btn-danger"
                  onClick={() => deletePhoto(photo.id)}
                >
                  Delete
                </button>
              </div>
              <div className="card-footer text-muted">
                {moment(photo.createdAt).fromNow()}
              </div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  private renderForm() {
    return (
      <form onSubmit={this.onSubmit.bind(this)} ref={this.formRef}>
        <fieldset disabled={this.props.isSubmitting}>
          <div className="mb-3">
            <label htmlFor="title" className="form-label">
              Title
            </label>
            <input
              type="text"
              required
              className="form-control"
              id="title"
              name="title"
              aria-describedby="title "
            />
          </div>
          <div className="mb-3">
            <label htmlFor="alt" className="form-label">
              Alt
            </label>
            <input type="text" className="form-control" name="alt" id="alt" />
          </div>
          <div className="mb-3">
            <label htmlFor="formFile" className="form-label">
              Image File
            </label>
            <input
              className="form-control"
              required
              type="file"
              name="formFile"
              id="formFile"
              accept="image/*"
            />
          </div>
          <button type="submit" className="btn btn-primary">
            Submit
          </button>
        </fieldset>
      </form>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.photo, // Selects which state properties are merged into the component's props
  PhotosStore.actionCreators // Selects which action creators are merged into the component's props
)(Photo as any);
