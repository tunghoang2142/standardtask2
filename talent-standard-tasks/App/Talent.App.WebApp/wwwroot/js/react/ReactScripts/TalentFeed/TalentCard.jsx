import React from 'react';
import ReactPlayer from 'react-player';
import PropTypes from 'prop-types'
import { Icon, Card, Grid, Image, GridColumn, Button, Label, CardContent } from 'semantic-ui-react'
import TalentCardDetail from './TalentCardDetail.jsx';

export default class TalentCard extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            feedData: this.props.feedData,
            toggleDetail: false
        }

        this.toggleDetailHandle = this.toggleDetailHandle.bind(this)
    }

    toggleDetailHandle() {
        this.setState({ toggleDetail: !this.state.toggleDetail })
    }

    render() {
        if (!!!this.props.feedData) return null
        return (
            this.props.feedData.map((feed) => {
                return (
                    <Card fluid key={feed.id}>
                        <Card.Content textAlign='left'>
                            <p className='profile-label talent-card-feed-header'>{feed.name}</p>
                            <Image floated='right'>
                                <Icon name='star' className='talent-card-feed-header' size='big' />
                            </Image>
                        </Card.Content>
                        {this.state.toggleDetail ?
                            <CardContent>
                                <TalentCardDetail feedData={feed} />
                            </CardContent>
                            :
                            <div>
                                <video controls className='talent-card-video'>
                                    {/* <source src="movie.mp4" type="video/mp4" />
                            <source src="movie.ogg" type="video/ogg" /> */}
                                    Your browser does not support the video player.
                                </video>

                            </div>
                        }
                        <Card.Content>
                            <Grid columns={4}>
                                <GridColumn>
                                    <Icon name={this.state.toggleDetail ? 'video' : 'user'} className='talent-card-feed-header' size='large'
                                        onClick={this.toggleDetailHandle} />
                                </GridColumn>
                                <GridColumn>
                                    <Icon name='file pdf outline' className='talent-card-feed-header' size='large' />
                                </GridColumn>
                                <GridColumn>
                                    <Icon name='linkedin' className='talent-card-feed-header' size='large' />
                                </GridColumn>
                                <GridColumn>
                                    <Icon name='github' className='talent-card-feed-header' size='large' />
                                </GridColumn>
                            </Grid>
                        </Card.Content>
                        {feed.skills ?
                            <Card.Content textAlign='left'>
                                {feed.skills.map((skill) => {
                                    return (
                                        <Label basic color='blue' key={skill}>{skill}</Label>
                                    )
                                })}
                            </Card.Content>
                            : null
                        }
                    </Card>
                )
            })
        )
    }
}

