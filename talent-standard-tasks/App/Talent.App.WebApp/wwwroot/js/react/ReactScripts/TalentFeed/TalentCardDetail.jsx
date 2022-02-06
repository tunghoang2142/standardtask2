import React from 'react';
import ReactDOM from 'react-dom';
import ReactPlayer from 'react-player';
import Cookies from 'js-cookie'
import { Grid, Image, GridColumn, Container } from 'semantic-ui-react'

export default class TalentCardDetail extends React.Component {
    constructor(props) {
        super(props)
        this.state = { image: null }
        this.init = this.init.bind(this)
    }

    componentDidMount() {
        this.init()
    }

    init() {
        // "kjiwwwyf.wrq.png"
        this.loadImage(this.props.feedData.photoId, (data) => {
            var reader = new FileReader()
            reader.onload = (event) => {
                this.setState({ image: event.target.result })
            }
            reader.readAsDataURL(data)
        })
    }

    loadImage(id, callback = (data) => { }) {
        var cookies = Cookies.get('talentAuthToken');
        const httpRequest = new XMLHttpRequest()
        var url = `http://localhost:60290/profile/profile/getImage/?id=${id}`

        httpRequest.open("GET", url, true);
        httpRequest.setRequestHeader('Authorization', 'Bearer ' + cookies)
        httpRequest.responseType = "blob";
        httpRequest.onload = () => {
            // console.log(httpRequest)
            callback(httpRequest.response)
        }
        httpRequest.onerror = () => {
            callback(httpRequest.response)
        }
        httpRequest.send()
    }

    render() {
        const { feedData } = this.props
        return (
            <Grid columns={2} >
                <GridColumn className='talent-feed-image-container'>
                    <Image src={this.state.image} className='talent-feed-image' />
                </GridColumn>
                <GridColumn textAlign='left'>
                    <p className='profile-label'>Talent snapshot</p>
                    <p>CURRENT EMPLOYMENT<br />{feedData.currentEmployment.split(' at ')[1]}</p>
                    <p>VISA STATUS<br />{feedData.visa}</p>
                    <p>POSITION<br />{feedData.currentEmployment.split(' at ')[0]}</p>
                </GridColumn>
            </Grid>
        )
    }
}