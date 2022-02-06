import React from 'react';
import ReactDOM from 'react-dom';
import Cookies from 'js-cookie'
import TalentCard from '../TalentFeed/TalentCard.jsx';
import { Loader, Image, Container } from 'semantic-ui-react';
import CompanyProfile from '../TalentFeed/CompanyProfile.jsx';
import FollowingSuggestion from '../TalentFeed/FollowingSuggestion.jsx';
import { BodyWrapper, loaderData } from '../Layout/BodyWrapper.jsx';

export default class TalentFeed extends React.Component {
    constructor(props) {
        super(props);

        let loader = loaderData
        loader.allowedUsers.push("Employer")
        loader.allowedUsers.push("Recruiter")

        this.state = {
            loadNumber: 5,
            loadPosition: 0,
            feedData: [],
            watchlist: [],
            loaderData: loader,
            loadingFeedData: true,
            companyDetails: null
        }

        this.init = this.init.bind(this);
    };

    init() {
        let loaderData = TalentUtil.deepCopy(this.state.loaderData)
        this.loadEmployerData((data) => {
            loaderData.isLoading = false;
            this.setState({ companyDetails: data }, () => {
                if (!!!this.state.loadingFeedData) {
                    this.setState({ loaderData })
                }
            })
        })

        this.loadTalentProfile((data) => {
            if (data.success) {
                this.setState({ feedData: data.data, loadingFeedData: false }, () => {
                    if (!!!loaderData.isLoading) {
                        this.setState({ loaderData })
                    }
                })
            }
        })
    }

    componentDidMount() {
        //window.addEventListener('scroll', this.handleScroll);
        this.init()
    };

    loadTalentProfile(callback = (data) => { }) {
        var cookies = Cookies.get('talentAuthToken');
        const httpRequest = new XMLHttpRequest()
        var url = `http://localhost:60290/profile/profile/getTalent`

        httpRequest.open("GET", url, true);
        httpRequest.setRequestHeader('Authorization', 'Bearer ' + cookies)
        httpRequest.onload = () => {
            callback(JSON.parse(httpRequest.response))
        }
        httpRequest.onerror = () => {
            callback(JSON.parse(httpRequest.response))
        }
        httpRequest.send()
    }

    loadEmployerData(callback = (data) => { }) {
        var cookies = Cookies.get('talentAuthToken');
        $.ajax({
            url: 'http://localhost:60290/profile/profile/getEmployerProfile',
            headers: {
                'Authorization': 'Bearer ' + cookies,
                'Content-Type': 'application/json'
            },
            type: "GET",
            contentType: "application/json",
            dataType: "json",
            success: function (res) {
                if (res.employer) {
                    // console.log(res.employer)
                    callback(res.employer)
                } else {
                    callback()
                }
            }.bind(this),
            error: function (res) {
                console.log(res.status)
                callback()
            }
        })
    }

    render() {
        return (
            <BodyWrapper reload={this.init} loaderData={this.state.loaderData}>
                <div className="ui grid talent-feed container">
                    <div className="four wide column">
                        <CompanyProfile data={this.state.companyDetails} />
                    </div>
                    <div className="eight wide column">
                        <Container textAlign='center'>
                            {this.state.feedData.length === 0 ?
                                <p>There are no talents found for your recruiment company </p>
                                :
                                <TalentCard feedData={this.state.feedData} />
                            }


                        </Container>
                    </div>
                    <div className="four wide column">
                        <div className="ui card">
                            <FollowingSuggestion />
                        </div>
                    </div>
                </div>
            </BodyWrapper>
        )
    }
}