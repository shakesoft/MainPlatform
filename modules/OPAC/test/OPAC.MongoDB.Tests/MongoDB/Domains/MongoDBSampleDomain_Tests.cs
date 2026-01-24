using OPAC.Samples;
using Xunit;

namespace OPAC.MongoDB.Domains;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleDomain_Tests : SampleManager_Tests<OPACMongoDbTestModule>
{

}
