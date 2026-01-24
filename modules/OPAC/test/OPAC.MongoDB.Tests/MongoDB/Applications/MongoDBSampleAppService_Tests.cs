using OPAC.MongoDB;
using OPAC.Samples;
using Xunit;

namespace OPAC.MongoDb.Applications;

[Collection(MongoTestCollection.Name)]
public class MongoDBSampleAppService_Tests : SampleAppService_Tests<OPACMongoDbTestModule>
{

}
