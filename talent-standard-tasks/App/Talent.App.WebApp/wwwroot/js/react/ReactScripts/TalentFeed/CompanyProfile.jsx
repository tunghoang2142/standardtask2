import React from 'react';
import { Card, Container, Icon, Image } from 'semantic-ui-react'

export default class CompanyProfile extends React.Component {
    constructor(props) {
        super(props)
    }

    render() {
        const { name, email, phone, location, skills } = this.props.data.companyContact
        return (
            <Card>
                <Card.Content textAlign='center'>
                    <Card.Header>
                        <Container>
                            <Image src='https://react.semantic-ui.com/images/wireframe/square-image.png' size='mini' circular />
                        </Container>
                        {name}
                    </Card.Header>
                    <Card.Meta>
                        <Icon name='marker' /> {location.city}, {location.country}
                    </Card.Meta>
                    <Card.Description>
                        {skills === undefined || skills.length === 0 ?
                            <p>We currently do not have specific skills that we desire.</p>
                            :
                            <p>We are in need of:
                                {skills.map((skill, i, skills) => {
                                    console.log(skill)
                                    if (i + 1 === skills.length) {
                                        return (' ' + skill)
                                    } else return (' ' + skill + ' ,')


                                })}
                            </p>
                        }

                    </Card.Description>
                </Card.Content>
                <Card.Content extra>
                    <Icon name='phone' />: {phone} <br />
                    <Icon name='mail' />: {email}
                </Card.Content>
            </Card>
        )
    }
}